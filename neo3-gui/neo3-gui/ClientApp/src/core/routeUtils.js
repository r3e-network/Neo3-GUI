/**
 * 路由工具函数
 * 统一处理路由参数获取，避免直接使用全局 location
 */

/**
 * 从路径中提取参数（冒号后的部分）
 * @param {string} pathname - 路径字符串
 * @returns {string} 提取的参数
 */
export const getRouteParam = (pathname) => {
  if (!pathname) return '';
  return pathname.split(':').pop() || '';
};

/**
 * 从 React Router location 或全局 location 获取参数
 * 优先使用 React Router 的 location
 * @param {object} routerLocation - React Router location 对象
 * @returns {string} 提取的参数
 */
export const getParamFromLocation = (routerLocation) => {
  const pathname = routerLocation?.pathname || window.location.pathname;
  return getRouteParam(pathname);
};

/**
 * 检查路径是否匹配指定模式
 * @param {string} pathname - 路径字符串
 * @param {string} pattern - 匹配模式（如 '/wallet/detail'）
 * @returns {boolean}
 */
export const matchPath = (pathname, pattern) => {
  if (!pathname) return false;
  return pathname.startsWith(pattern);
};

const routeUtils = {
  getRouteParam,
  getParamFromLocation,
  matchPath
};

export default routeUtils;
