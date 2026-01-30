/**
 * Config Manager for Neo3-GUI
 * Supports both Electron and Tauri environments
 */

import { isTauri } from './platform';

let RPCURL = "";
let WSURL = "";

class Config {
  constructor() {
    this.Host = "localhost";
    this.Port = 8081;
    this.Language = "";
    this.Network = "mainnet";
    this.initialized = false;
    
    // Initialize synchronously for Electron, async for Tauri
    if (!isTauri()) {
      this.initElectron();
    }
  }

  initElectron() {
    try {
      const fs = require("fs");
      const path = require("path");
      const { app } = require('@electron/remote');
      
      const appPath = app.getAppPath().replace("app.asar", "");
      const configPath = path.join(appPath, "gui-config.json");
      
      const file = fs.readFileSync(configPath, "utf8");
      const config = JSON.parse(file);
      this.initConfig(config);
      this.initialized = true;
    } catch (error) {
      console.error('Config init error:', error);
      this.initConfig({});
    }
  }

  async initTauri() {
    if (this.initialized) return;
    
    try {
      const { invoke } = await import('@tauri-apps/api/core');
      const config = await invoke('read_config');
      this.initConfig(config);
      this.initialized = true;
    } catch (error) {
      console.error('Failed to read config:', error);
      this.initConfig({});
    }
  }

  initConfig = (config) => {
    this.Host = config.Host || "localhost";
    this.Port = config.Port || 8081;
    RPCURL = "http://" + this.Host + ":" + this.Port;
    WSURL = "ws://" + this.Host + ":" + this.Port;
    this.Language = config.Language || "";
    this.Network = config.Network || "mainnet";
  };

  saveConfig = () => {
    if (isTauri()) {
      this.saveConfigTauri();
    } else {
      this.saveConfigElectron();
    }
  };

  saveConfigElectron() {
    try {
      const fs = require("fs");
      const path = require("path");
      const { app } = require('@electron/remote');
      
      const appPath = app.getAppPath().replace("app.asar", "");
      const configPath = path.join(appPath, "gui-config.json");
      
      const json = JSON.stringify(this, null, 4);
      fs.writeFile(configPath, json, (err) => {
        if (err) console.error(err);
      });
    } catch (error) {
      console.error('Failed to save config:', error);
    }
  }

  async saveConfigTauri() {
    try {
      const { invoke } = await import('@tauri-apps/api/core');
      await invoke('write_config', {
        config: {
          Host: this.Host,
          Port: this.Port,
          Language: this.Language,
          Network: this.Network
        }
      });
    } catch (error) {
      console.error('Failed to save config:', error);
    }
  }

  changeLang = (lng) => {
    this.Language = lng;
    this.saveConfig();
  };

  changeNetwork = (network) => {
    this.Network = network;
    this.saveConfig();
  };

  getRpcUrl = () => RPCURL;
  getWsUrl = () => WSURL;
}

const config = new Config();

// Initialize Tauri config asynchronously
if (isTauri()) {
  config.initTauri();
}

export default config;
