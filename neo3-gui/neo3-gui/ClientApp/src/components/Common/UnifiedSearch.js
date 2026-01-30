/* eslint-disable */
/**
 * UnifiedSearch - 统一搜索组件
 * 替代重复的 searcharea.js 文件
 * 
 * 用法:
 * <UnifiedSearch 
 *   type="contract|transaction|chain" 
 *   placeholder="自定义提示文本(可选)"
 * />
 */
import React, { createRef } from "react";
import { Input, message } from "antd";
import { ArrowRightOutlined, SearchOutlined } from "@ant-design/icons";
import { withTranslation } from "react-i18next";
import { postAsync } from "../../core/request";
import Topath from "./topath";

// 搜索类型配置
const SEARCH_CONFIGS = {
  contract: {
    placeholder: "search.hash-hint",
    apiMethod: "GetContract",
    getParams: (input) => ({ contractHash: input }),
    getPath: (input, response) => `/contract/detail:${input}`,
    validateResponse: (response) => response.msgType !== -1,
    errorKey: "search.hash unexist"
  },
  transaction: {
    placeholder: "search.hash-hint",
    apiMethod: "GetTransaction",
    getParams: (input) => ({ txId: input }),
    getPath: (input, response) => `transaction:${input}`,
    validateResponse: (response) => response.msgType === 3,
    errorKey: "search.check again"
  },
  chain: {
    placeholder: "search.chain-hint",
    apiMethod: null, // 动态决定
    getParams: (input) => {
      if (input.length === 66) {
        return { method: "GetBlockByHash", params: { hash: input } };
      }
      return { method: "GetBlock", params: { index: Number(input) } };
    },
    getPath: (input, response) => `/chain/detail:${response.result.blockHeight}`,
    validateResponse: (response) => response.msgType !== -1,
    errorKey: "blockchain.search input-invalid"
  }
};

@withTranslation()
class UnifiedSearch extends React.Component {
  constructor(props) {
    super(props);
    this.searchInput = createRef();
    this.state = {
      disabled: false,
      cname: "search-content",
      topath: ""
    };
  }

  componentWillUnmount() {
    document.removeEventListener("click", this.removeClass);
  }

  addClass = (e) => {
    this.stopPropagation(e);
    this.setState({
      cname: "search-content height-sea show-child",
      disabled: true,
    });
    document.addEventListener("click", this.removeClass);
  };

  removeClass = () => {
    if (this.state.disabled) {
      this.setState({
        cname: "search-content height-sea",
        disabled: false,
      });
    }
    document.removeEventListener("click", this.removeClass);
    setTimeout(() => {
      this.setState({
        cname: "search-content",
        disabled: false,
      });
    }, 500);
  };

  stopPropagation(e) {
    e.nativeEvent.stopImmediatePropagation();
  }

  handleSearch = async () => {
    const { t, type = "contract" } = this.props;
    const config = SEARCH_CONFIGS[type];
    
    if (!config) {
      console.error(`Unknown search type: ${type}`);
      return;
    }

    const input = this.searchInput.current.input.value.trim();
    
    if (!input) {
      message.info(t("search.check again"));
      return;
    }

    try {
      let response;
      
      if (type === "chain") {
        // Chain 搜索需要动态决定 API 方法
        const { method, params } = config.getParams(input);
        response = await postAsync(method, params);
      } else {
        response = await postAsync(config.apiMethod, config.getParams(input));
      }

      if (!config.validateResponse(response)) {
        message.info(t(config.errorKey));
        return;
      }

      this.setState({ topath: config.getPath(input, response) });
    } catch (error) {
      console.error("Search error:", error);
      message.error(t("search.error"));
    }
  };

  render() {
    const { t, type = "contract", placeholder } = this.props;
    const config = SEARCH_CONFIGS[type];
    const placeholderText = placeholder || t(config?.placeholder || "search.hash-hint");

    return (
      <div className="search-area">
        <Topath topath={this.state.topath} />
        <div className="search-btn">
          <SearchOutlined className="inset-btn" onClick={this.addClass} />
        </div>
        <div className={this.state.cname}>
          <div className="search-detail" onClick={this.stopPropagation}>
            <Input
              placeholder={placeholderText}
              onPressEnter={this.handleSearch}
              ref={this.searchInput}
              suffix={<ArrowRightOutlined onClick={this.handleSearch} />}
            />
          </div>
        </div>
      </div>
    );
  }
}

export default UnifiedSearch;
