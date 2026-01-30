/**
 * Platform detection and unified API for Neo3-GUI
 * Supports both Electron and Tauri environments
 */

// Detect runtime environment
export const isTauri = () => {
  return typeof window !== 'undefined' && window.__TAURI__ !== undefined;
};

export const isElectron = () => {
  return typeof window !== 'undefined' && 
         typeof window.process === 'object' && 
         window.process.type === 'renderer';
};

// Lazy-loaded platform APIs to avoid build-time resolution issues
let _platformDialog = null;
let _platformShell = null;
let _platformApp = null;
let _initialized = false;

const initPlatform = async () => {
  if (_initialized) return;
  
  if (isTauri()) {
    // Use Tauri APIs - dynamic import
    const tauriApi = await import('./tauri-api');
    _platformDialog = tauriApi.dialog;
    _platformShell = tauriApi.shell;
    _platformApp = tauriApi.app;
  } else if (isElectron()) {
    // Use Electron APIs
    try {
      const remote = window.require('@electron/remote');
      const electron = window.require('electron');
      _platformDialog = remote.dialog;
      _platformShell = electron.shell;
      _platformApp = remote.app;
    } catch (e) {
      console.warn('Electron APIs not available:', e);
      setFallback();
    }
  } else {
    setFallback();
  }
  
  _initialized = true;
};

const setFallback = () => {
  _platformDialog = {
    showOpenDialog: async () => ({ filePaths: [], canceled: true }),
    showSaveDialog: async () => ({ filePath: undefined, canceled: true })
  };
  _platformShell = {
    openExternal: (url) => window.open(url, '_blank')
  };
  _platformApp = {
    getVersion: () => '1.6.0',
    getAppPath: () => ''
  };
};

// Initialize on first access
setFallback(); // Set fallback as default

// Proxy objects that initialize on first use
export const dialog = {
  async showOpenDialog(options) {
    await initPlatform();
    return _platformDialog.showOpenDialog(options);
  },
  async showSaveDialog(options) {
    await initPlatform();
    return _platformDialog.showSaveDialog(options);
  }
};

export const shell = {
  async openExternal(url) {
    await initPlatform();
    return _platformShell.openExternal(url);
  }
};

export const app = {
  async getVersion() {
    await initPlatform();
    return _platformApp.getVersion();
  },
  getAppPath() {
    return _platformApp?.getAppPath?.() || '';
  }
};

export default { dialog, shell, app, isTauri, isElectron };
