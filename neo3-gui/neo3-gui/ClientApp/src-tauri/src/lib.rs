use serde::{Deserialize, Serialize};
use std::process::{Child, Command, Stdio};
use std::sync::Mutex;
use tauri::{AppHandle, Manager, State};

mod config;

// State to hold the Neo node process
pub struct NeoNodeState(pub Mutex<Option<Child>>);

#[derive(Debug, Serialize, Deserialize)]
pub struct BackendStatus {
    running: bool,
    pid: Option<u32>,
}

#[tauri::command]
pub fn start_backend(
    app: AppHandle,
    state: State<NeoNodeState>,
    network: Option<String>,
    port: Option<u16>,
) -> Result<BackendStatus, String> {
    let mut node = state.0.lock().map_err(|e| e.to_string())?;

    // Kill existing process if any
    if let Some(mut child) = node.take() {
        let _ = child.kill();
    }

    // Get the resource path for the backend
    let resource_path = app
        .path()
        .resource_dir()
        .map_err(|e| e.to_string())?
        .join("build-neo-node");

    // Determine the executable name based on platform
    #[cfg(target_os = "windows")]
    let exe_name = "neo3-gui.exe";
    #[cfg(not(target_os = "windows"))]
    let exe_name = "neo3-gui";

    let exe_path = resource_path.join(exe_name);

    if !exe_path.exists() {
        return Err(format!("Backend executable not found: {:?}", exe_path));
    }

    // Build the command
    let mut cmd = Command::new(&exe_path);
    cmd.current_dir(&resource_path)
        .stdout(Stdio::piped())
        .stderr(Stdio::piped());

    // Set environment variables
    if let Some(net) = network {
        cmd.env("NEO_NETWORK", net);
    }
    if let Some(p) = port {
        cmd.env("NEO_GUI_PORT", p.to_string());
    }

    // Add PATH for macOS dotnet
    #[cfg(target_os = "macos")]
    {
        if let Ok(path) = std::env::var("PATH") {
            cmd.env("PATH", format!("{}:/usr/local/share/dotnet", path));
        }
    }

    // Spawn the process
    let child = cmd
        .spawn()
        .map_err(|e| format!("Failed to start backend: {}", e))?;
    let pid = child.id();

    *node = Some(child);

    log::info!("Neo backend started with PID: {}", pid);

    Ok(BackendStatus {
        running: true,
        pid: Some(pid),
    })
}

#[tauri::command]
pub fn stop_backend(state: State<NeoNodeState>) -> Result<BackendStatus, String> {
    let mut node = state.0.lock().map_err(|e| e.to_string())?;

    if let Some(mut child) = node.take() {
        child
            .kill()
            .map_err(|e| format!("Failed to kill backend: {}", e))?;
        log::info!("Neo backend stopped");
    }

    Ok(BackendStatus {
        running: false,
        pid: None,
    })
}

#[tauri::command]
pub fn get_backend_status(state: State<NeoNodeState>) -> Result<BackendStatus, String> {
    let mut node = state.0.lock().map_err(|e| e.to_string())?;

    if let Some(ref mut child) = *node {
        match child.try_wait() {
            Ok(Some(_)) => {
                // Process has exited
                *node = None;
                Ok(BackendStatus {
                    running: false,
                    pid: None,
                })
            }
            Ok(None) => {
                // Process is still running
                Ok(BackendStatus {
                    running: true,
                    pid: Some(child.id()),
                })
            }
            Err(e) => Err(format!("Failed to check backend status: {}", e)),
        }
    } else {
        Ok(BackendStatus {
            running: false,
            pid: None,
        })
    }
}

#[tauri::command]
pub fn get_app_version() -> String {
    env!("CARGO_PKG_VERSION").to_string()
}

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
    tauri::Builder::default()
        .plugin(tauri_plugin_shell::init())
        .plugin(tauri_plugin_dialog::init())
        .plugin(tauri_plugin_fs::init())
        .plugin(tauri_plugin_process::init())
        .manage(NeoNodeState(Mutex::new(None)))
        .invoke_handler(tauri::generate_handler![
            start_backend,
            stop_backend,
            get_backend_status,
            get_app_version,
            config::read_config,
            config::write_config,
            config::get_config_path,
        ])
        .setup(|app| {
            if cfg!(debug_assertions) {
                app.handle().plugin(
                    tauri_plugin_log::Builder::default()
                        .level(log::LevelFilter::Info)
                        .build(),
                )?;
            }

            #[cfg(not(any(target_os = "android", target_os = "ios")))]
            {
                app.handle()
                    .plugin(tauri_plugin_single_instance::init(|app, _args, _cwd| {
                        if let Some(window) = app.get_webview_window("main") {
                            let _ = window.set_focus();
                        }
                    }))?;
            }

            Ok(())
        })
        .on_window_event(|window, event| {
            if let tauri::WindowEvent::CloseRequested { .. } = event {
                // Stop the backend when window is closed
                if let Some(state) = window.try_state::<NeoNodeState>() {
                    if let Ok(mut node) = state.0.lock() {
                        if let Some(mut child) = node.take() {
                            let _ = child.kill();
                            log::info!("Neo backend stopped on window close");
                        }
                    }
                }
            }
        })
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}
