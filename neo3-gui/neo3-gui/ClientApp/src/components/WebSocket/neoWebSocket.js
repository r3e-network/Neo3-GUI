import Config from "../../config";

class NeoWebSocket {
  constructor() {
    this.processMethods = {};
    this.lock = false;
    this.ws = null;
  }

  initWebSocket = () => {
    try {
      this.ws = this.createWebSocket();
    } catch (error) {
      console.warn("NeoWebSocket=>", error);
    }
  };

  createWebSocket = () => {
    let ws = new WebSocket(Config.getWsUrl());

    ws.onclose = (e) => {
      console.warn("NeoWebSocket=> [closed]", e);
      this.reconnectWebSocket();
    };

    ws.onerror = (e) => {
      console.warn("NeoWebSocket=> [error]", e);
    };

    ws.onmessage = this.processMessage;
    return ws;
  };

  reconnectWebSocket = () => {
    if (this.lock) {
      return;
    }
    this.lock = true;
    setTimeout(() => {
      this.initWebSocket();
      this.lock = false;
    }, 5000);
  };

  /**
   * distribute websocket message to registered process methods
   */
  processMessage = (message) => {
    let msg = JSON.parse(message.data);
    let methods = this.processMethods[msg.method];
    if (methods && methods.length > 0) {
      for (let methodFunc of methods) {
        try {
          methodFunc(msg);
        } catch (error) {
          console.error("NeoWebSocket=>", error);
        }
      }
    }
  };

  /**
   * register new process method
   * @param {*} method message method name
   * @param {*} func process method
   */
  registMethod(method, func) {
    let methods = this.processMethods[method];
    if (!(methods && methods.length)) {
      methods = [];
    }
    methods.push(func);
    this.processMethods[method] = methods;
  }

  /**
   * unregister process method
   * @param {*} method
   * @param {*} func
   */
  unregistMethod(method, func) {
    let methods = this.processMethods[method];
    if (methods && methods.length) {
      let i = 0;
      while (i < methods.length) {
        if (methods[i] === func) {
          methods.splice(i, 1);
        } else {
          ++i;
        }
      }
      this.processMethods[method] = methods;
    }
  }
}

const instance = new NeoWebSocket();
export default instance;
