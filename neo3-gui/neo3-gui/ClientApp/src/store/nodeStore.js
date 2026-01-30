import { action, makeObservable } from "mobx";
import neoNode from "../neonode";

class NodeStore {
  constructor() {
    makeObservable(this);
    this.nodeManager = neoNode;
  }

  @action start(network, port) {
    this.nodeManager.start(network, port);
  }

  @action kill(data) {
    this.nodeManager.kill();
  }
}

export default NodeStore;
