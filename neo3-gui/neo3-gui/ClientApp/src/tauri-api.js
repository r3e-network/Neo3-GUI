/**
 * Tauri API wrapper for Neo3-GUI
 * Replaces Electron API calls with Tauri equivalents
 */

// Inline check to avoid circular dependency with platform.js
const isTauriEnv = () => typeof window !== 'undefined' && window.__TAURI__ !== undefined;

// Dialog API
export const dialog = {
  async showOpenDialog(options = {}) {
    if (!isTauriEnv()) {
      console.warn('Not running in Tauri environment');
      return { filePaths: [] };
    }
    
    const { open } = await import('@tauri-apps/plugin-dialog');
    
    const filters = options.filters?.map(f => ({
      name: f.name,
      extensions: f.extensions
    })) || [];
    
    const selected = await open({
      title: options.title,
      defaultPath: options.defaultPath,
      filters: filters,
      multiple: options.properties?.includes('multiSelections') || false,
      directory: options.properties?.includes('openDirectory') || false,
    });
    
    // Normalize result to match Electron format
    if (selected === null) {
      return { filePaths: [], canceled: true };
    }
    
    const filePaths = Array.isArray(selected) ? selected : [selected];
    return { filePaths, canceled: false };
  },
  
  async showSaveDialog(options = {}) {
    if (!isTauriEnv()) {
      console.warn('Not running in Tauri environment');
      return { filePath: undefined };
    }
    
    const { save } = await import('@tauri-apps/plugin-dialog');
    
    const filters = options.filters?.map(f => ({
      name: f.name,
      extensions: f.extensions
    })) || [];
    
    const filePath = await save({
      title: options.title,
      defaultPath: options.defaultPath,
      filters: filters,
    });
    
    return { filePath, canceled: filePath === null };
  }
};

// Shell API
export const shell = {
  async openExternal(url) {
    if (!isTauriEnv()) {
      window.open(url, '_blank');
      return;
    }
    
    const { open } = await import('@tauri-apps/plugin-shell');
    await open(url);
  }
};

// App API
export const app = {
  async getVersion() {
    if (!isTauriEnv()) {
      return '1.6.0';
    }
    
    const { invoke } = await import('@tauri-apps/api/core');
    return await invoke('get_app_version');
  },
  
  getAppPath() {
    // In Tauri, this is handled differently
    // Return empty string as paths are managed by Rust backend
    return '';
  }
};

// Export for compatibility
export default {
  dialog,
  shell,
  app,
  isTauri: isTauriEnv
};
