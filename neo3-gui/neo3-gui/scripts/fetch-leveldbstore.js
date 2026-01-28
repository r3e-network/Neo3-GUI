const fs = require('fs');
const path = require('path');
const os = require('os');
const https = require('https');
const { execFileSync } = require('child_process');

const NEO_VERSION = '3.9.1';
const PACKAGE_ID = 'neo.plugins.storage.leveldbstore';
const DLL_RELATIVE_PATH = 'lib/net10.0/LevelDBStore.dll';

function getNupkgUrl() {
  return `https://api.nuget.org/v3-flatcontainer/${PACKAGE_ID}/${NEO_VERSION}/${PACKAGE_ID}.${NEO_VERSION}.nupkg`;
}

function getDllPath() {
  return path.join(__dirname, '..', 'Plugins', 'LevelDBStore', 'LevelDBStore.dll');
}

function downloadFile(url, destPath) {
  return new Promise((resolve, reject) => {
    const request = https.get(url, (response) => {
      if (response.statusCode >= 300 && response.statusCode < 400 && response.headers.location) {
        response.resume();
        return resolve(downloadFile(response.headers.location, destPath));
      }
      if (response.statusCode !== 200) {
        response.resume();
        return reject(new Error(`Download failed: ${response.statusCode} ${response.statusMessage}`));
      }

      const fileStream = fs.createWriteStream(destPath);
      response.pipe(fileStream);
      fileStream.on('finish', () => fileStream.close(resolve));
      fileStream.on('error', reject);
    });
    request.on('error', reject);
  });
}

function extractDllBuffer(nupkgPath, tempDir) {
  const entry = DLL_RELATIVE_PATH.replace(/\\/g, '/');
  try {
    const data = execFileSync('unzip', ['-p', nupkgPath, entry], {
      stdio: ['ignore', 'pipe', 'pipe'],
    });
    if (!data || data.length === 0) {
      throw new Error('Unzip returned empty data');
    }
    return data;
  } catch (err) {
    if (process.platform !== 'win32') {
      throw new Error(`Failed to extract ${entry}. Ensure 'unzip' is installed. ${err.message}`);
    }
  }

  const extractDir = path.join(tempDir, 'extract');
  execFileSync(
    'powershell',
    [
      '-NoProfile',
      '-Command',
      `Expand-Archive -Path \"${nupkgPath}\" -DestinationPath \"${extractDir}\" -Force`,
    ],
    { stdio: 'inherit' }
  );

  const dllPath = path.join(extractDir, ...DLL_RELATIVE_PATH.split('/'));
  if (!fs.existsSync(dllPath)) {
    throw new Error(`Extracted DLL not found at ${dllPath}`);
  }
  return fs.readFileSync(dllPath);
}

async function fetchLevelDbStore({ dryRun = false } = {}) {
  const url = getNupkgUrl();
  const targetPath = getDllPath();

  if (dryRun) {
    console.log(`URL: ${url}`);
    console.log(`Target: ${targetPath}`);
    return;
  }

  const tempRoot = fs.mkdtempSync(path.join(os.tmpdir(), 'neo3-gui-'));
  const nupkgPath = path.join(tempRoot, `${PACKAGE_ID}.${NEO_VERSION}.nupkg`);
  const backupPath = path.join(tempRoot, 'LevelDBStore.dll.bak');

  try {
    await downloadFile(url, nupkgPath);
    const dllBuffer = extractDllBuffer(nupkgPath, tempRoot);

    fs.mkdirSync(path.dirname(targetPath), { recursive: true });
    if (fs.existsSync(targetPath)) {
      fs.copyFileSync(targetPath, backupPath);
    }
    fs.writeFileSync(targetPath, dllBuffer);
    console.log(`Updated ${targetPath}`);
  } finally {
    fs.rmSync(tempRoot, { recursive: true, force: true });
  }
}

if (require.main === module) {
  const dryRun = process.argv.includes('--dry-run');
  fetchLevelDbStore({ dryRun }).catch((err) => {
    console.error(err.message);
    process.exit(1);
  });
}

module.exports = {
  NEO_VERSION,
  PACKAGE_ID,
  getNupkgUrl,
  getDllPath,
  fetchLevelDbStore,
};
