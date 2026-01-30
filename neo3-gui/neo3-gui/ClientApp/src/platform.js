/**
 * Platform detection and unified API for Neo3-GUI
 * Supports both Electron and Tauri environments
 */

// Detect runtime environment
export const isTauri = () => window.__TAURI__ !== undefined;
export const isElectron = () => {
  return typeof window !== 'undefined' && 
         typeof window.process === 'object' && 
         window.process.type === 'renderer';
};

// Export the appropriate modules based on platform
let platformDialog, platformShell, platformApp;

if (isTauri()) {
  // Use Tauri APIs
  const tauriApi = require('./tauri-api');
  platformDialog = tauriApi.dialog;
  platformShell = tauriApi.shell;
  platformApp = tauriApi.app;
} else if (isElectron()) {
  // Use Electron APIs
  const remote = require('@electron/remote');
  const electron = require('electron');
  platformDialog = remote.dialog;
  platformShell = electron.shell;
  platformApp = remote.app;
} else {
  // Fallback for web/development
  platformDialog = {
    showOpenDialog: async () => ({ filePaths: [], canceled: true }),
    showSaveDialog: async () => ({ filePath: undefined, canceled: true })
  };
  platformShell = {
    openExternal: (url) => window.open(url, '_blank')
  };
  platformApp = {
    getVersion: () => '1.6.0',
    getAppPath: () => ''
  };
}

export const dialog = platformDialog;
export const shell = platformShell;
export const app = platformApp;

export default { dialog, shell, app, isTauri, isElectron };
