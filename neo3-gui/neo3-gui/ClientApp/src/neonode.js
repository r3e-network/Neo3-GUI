/**
 * Neo Node Manager for Neo3-GUI
 * Supports both Electron and Tauri environments
 */

import Config from "./config";
import { isTauri } from "./platform";

const isMac = typeof process !== 'undefined' && process.platform === "darwin";
const isWin = typeof process !== 'undefined' && process.platform === "win32";

class NeoNode {
  constructor() {
    this.pendingSwitchTimer = null;
    this.node = null;
  }

  debounce = (fn, wait) => {
    if (this.pendingSwitchTimer !== null) {
      clearTimeout(this.pendingSwitchTimer);
    }
    this.pendingSwitchTimer = setTimeout(fn, wait);
  };

  kill() {
    if (isTauri()) {
      this.killTauri();
    } else {
      this.killElectron();
    }
  }

  killElectron() {
    if (this.node) {
      this.node.kill();
      this.node = null;
    }
  }

  async killTauri() {
    try {
      const { invoke } = await import('@tauri-apps/api/core');
      await invoke('stop_backend');
    } catch (error) {
      console.error('Failed to stop backend:', error);
    }
  }

  start(env, errorCallback) {
    if (isTauri()) {
      this.startTauri(env, errorCallback);
    } else {
      this.startElectron(env, errorCallback);
    }
  }

  startElectron(env, errorCallback) {
    const { spawn } = require("child_process");
    const path = require("path");
    const { app } = require('@electron/remote');
    
    const appPath = app.getAppPath();
    const startPath = appPath.replace("app.asar", "");
    
    const parentEnv = process.env;
    const childEnv = { ...parentEnv, ...env };
    
    if (isMac) {
      childEnv.PATH = childEnv.PATH + ":/usr/local/share/dotnet";
    }

    const command = isWin ? "./neo3-gui.exe" : "./neo3-gui";
    
    const ps = spawn(command, [], {
      shell: false,
      encoding: "utf8",
      cwd: path.join(startPath, "build-neo-node"),
      env: childEnv,
    });
    
    ps.firstError = true;
    ps.stdout.on("data", () => {
      // stdout data ignored
    });
    ps.stderr.setEncoding("utf8");
    ps.stderr.on("data", (data) => {
      console.error(ps.pid + ":" + data.toString());
      if (ps.firstError && errorCallback) {
        ps.firstError = false;
        errorCallback(data.toString());
      }
    });
    ps.env = env;
    this.node = ps;
  }

  async startTauri(env, errorCallback) {
    try {
      const { invoke } = await import('@tauri-apps/api/core');
      await invoke('start_backend', {
        network: env?.NEO_NETWORK || null,
        port: env?.NEO_GUI_PORT ? parseInt(env.NEO_GUI_PORT) : null
      });
    } catch (error) {
      console.error('Failed to start backend:', error);
      if (errorCallback) {
        errorCallback(error.toString());
      }
    }
  }

  startNode(network, port, errorCallback) {
    const env = { NEO_NETWORK: network || "", NEO_GUI_PORT: port || "" };
    this.start(env, errorCallback);
  }

  switchNode(network) {
    if (network) {
      Config.changeNetwork(network);
    }

    let retryCount = 0;
    this.delayStartNode(retryCount);
  }

  delayStartNode(retryCount) {
    retryCount = retryCount || 0;
    if (retryCount > 10) {
      return;
    }
    this.kill();
    this.debounce(() => {
      this.startNode(Config.Network, Config.Port, () => {
        retryCount++;
        this.delayStartNode(retryCount);
      });
    }, 1000);
  }
}

const singleton = new NeoNode();
export default singleton;
