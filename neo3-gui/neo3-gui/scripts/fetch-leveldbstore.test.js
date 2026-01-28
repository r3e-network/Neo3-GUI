const assert = require('assert');

const fetcher = require('./fetch-leveldbstore');

const expectedVersion = '3.9.1';
const expectedPackageId = 'neo.plugins.storage.leveldbstore';
const expectedUrl = `https://api.nuget.org/v3-flatcontainer/${expectedPackageId}/${expectedVersion}/${expectedPackageId}.${expectedVersion}.nupkg`;

assert.strictEqual(fetcher.NEO_VERSION, expectedVersion);
assert.strictEqual(fetcher.PACKAGE_ID, expectedPackageId);
assert.strictEqual(fetcher.getNupkgUrl(), expectedUrl);

const dllPath = fetcher.getDllPath();
assert.ok(dllPath.endsWith('neo3-gui/neo3-gui/Plugins/LevelDBStore/LevelDBStore.dll'));

console.log('fetch-leveldbstore test passed');
