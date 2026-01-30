/**
 * Neo Node Manager for Tauri
 * Replaces Electron's child_process with Tauri commands
 */

import { invoke } from '@tauri-apps/api/core';

class NeoNode {
  constructor() {
    this.pendingSwitchTimer = null;
    this.isRunning = false;
  }

  debounce = (fn, wait) => {
    if (this.pendingSwitchTimer !== null) {
      clearTimeout(this.pendingSwitchTimer);
    }
    this.pendingSwitchTimer = setTimeout(fn, wait);
  };

  async kill() {
    try {
      await invoke('stop_backend');
      this.isRunning = false;
    } catch (error) {
      console.error('Failed to stop backend:', error);
    }
  }

  async start(network, port, errorCallback) {
    try {
      const result = await invoke('start_backend', {
        network: network || null,
        port: port || null
      });
      this.isRunning = result.running;
      console.log('Backend started:', result);
    } catch (error) {
      console.error('Failed to start backend:', error);
      if (errorCallback) {
        errorCallback(error.toString());
      }
    }
  }

  async startNode(network, port, errorCallback) {
    await this.start(network, port, errorCallback);
  }

  async getStatus() {
    try {
      return await invoke('get_backend_status');
    } catch (error) {
      console.error('Failed to get backend status:', error);
      return { running: false, pid: null };
    }
  }

  /**
   * Force restart node after 1 second (using config file)
   */
  async switchNode(network) {
    console.log("switch to:", network);
    
    // Update config via Tauri command
    if (network) {
      try {
        const config = await invoke('read_config');
        config.Network = network;
        await invoke('write_config', { config });
      } catch (error) {
        console.error('Failed to update config:', error);
      }
    }

    let retryCount = 0;
    this.delayStartNode(retryCount, network);
  }

  delayStartNode(retryCount, network) {
    retryCount = retryCount || 0;
    if (retryCount > 10) {
      console.log("stop retry");
      return;
    }
    
    this.debounce(async () => {
      await this.kill();
      await this.startNode(network, null, () => {
        console.log(
          new Date(),
          retryCount + ":switch network fail:" + network
        );
        retryCount++;
        this.delayStartNode(retryCount, network);
      });
    }, 1000);
  }
}

const singleton = new NeoNode();
export default singleton;
