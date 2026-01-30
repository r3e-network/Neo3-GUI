use serde::{Deserialize, Serialize};
use std::fs;
use std::path::PathBuf;
use tauri::{AppHandle, Manager};

#[derive(Debug, Serialize, Deserialize, Clone)]
#[serde(rename_all = "PascalCase")]
pub struct GuiConfig {
    #[serde(default = "default_host")]
    pub host: String,
    #[serde(default = "default_port")]
    pub port: u16,
    #[serde(default)]
    pub language: String,
    #[serde(default = "default_network")]
    pub network: String,
}

fn default_host() -> String {
    "localhost".to_string()
}

fn default_port() -> u16 {
    8081
}

fn default_network() -> String {
    "mainnet".to_string()
}

impl Default for GuiConfig {
    fn default() -> Self {
        Self {
            host: default_host(),
            port: default_port(),
            language: String::new(),
            network: default_network(),
        }
    }
}

fn get_config_file_path(app: &AppHandle) -> Result<PathBuf, String> {
    let resource_path = app.path().resource_dir().map_err(|e| e.to_string())?;
    Ok(resource_path.join("gui-config.json"))
}

#[tauri::command]
pub fn get_config_path(app: AppHandle) -> Result<String, String> {
    let path = get_config_file_path(&app)?;
    Ok(path.to_string_lossy().to_string())
}

#[tauri::command]
pub fn read_config(app: AppHandle) -> Result<GuiConfig, String> {
    let config_path = get_config_file_path(&app)?;

    if !config_path.exists() {
        return Ok(GuiConfig::default());
    }

    let content =
        fs::read_to_string(&config_path).map_err(|e| format!("Failed to read config: {}", e))?;

    let config: GuiConfig =
        serde_json::from_str(&content).map_err(|e| format!("Failed to parse config: {}", e))?;

    Ok(config)
}

#[tauri::command]
pub fn write_config(app: AppHandle, config: GuiConfig) -> Result<(), String> {
    let config_path = get_config_file_path(&app)?;

    let content = serde_json::to_string_pretty(&config)
        .map_err(|e| format!("Failed to serialize config: {}", e))?;

    fs::write(&config_path, content).map_err(|e| format!("Failed to write config: {}", e))?;

    Ok(())
}
