const path = require('path');

module.exports = {
  app: {
    getAppPath: () => process.cwd(),
    isPackaged: false,
  },
  dialog: {
    showOpenDialog: () => Promise.resolve({ filePaths: [] }),
    showSaveDialog: () => Promise.resolve({ filePath: path.join(process.cwd(), 'mock-file') }),
    showErrorBox: () => {},
  },
};
