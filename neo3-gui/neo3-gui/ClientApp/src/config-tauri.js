/**
 * Config Manager for Tauri
 * Replaces Electron's fs operations with Tauri commands
 */

import { invoke } from '@tauri-apps/api/core';

let RPCURL = "";
let WSURL = "";

class Config {
  constructor() {
    this.Host = "localhost";
    this.Port = 8081;
    this.Language = "";
    this.Network = "mainnet";
    this.initialized = false;
  }

  async init() {
    if (this.initialized) return;
    
    try {
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

  async saveConfig() {
    try {
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

  async changeLang(lng) {
    this.Language = lng;
    await this.saveConfig();
  }

  async changeNetwork(network) {
    this.Network = network;
    await this.saveConfig();
  }

  getRpcUrl = () => RPCURL;
  getWsUrl = () => WSURL;
}

const config = new Config();

// Initialize config asynchronously
config.init();

export default config;
