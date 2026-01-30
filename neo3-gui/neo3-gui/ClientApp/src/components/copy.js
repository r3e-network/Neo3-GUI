import React from "react";
import { CopyOutlined } from '@ant-design/icons';
import { message } from 'antd';
import { useTranslation } from 'react-i18next';

const Copy = (props) => {
  const { msg } = props;
  const { t } = useTranslation();
  const clickCopy = () => {
    if (msg !== "") {
      const ele = document.createElement("textarea");
      ele.value = msg;
      ele.style.opacity = "0";
      document.body.appendChild(ele);
      ele.select();
      document.execCommand("copy");
      message.success(t("common.copied"));
      document.body.removeChild(ele);
    }
  };
  return (
    <a className="clipboard" onClick={clickCopy}><CopyOutlined /></ a>
  );
};

export { Copy };