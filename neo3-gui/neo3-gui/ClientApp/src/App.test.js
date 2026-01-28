import { render } from '@testing-library/react';
import App from './App';

jest.mock('./neonode', () => ({
  switchNode: jest.fn(),
}));

jest.mock('./components/WebSocket/neoWebSocket', () => ({
  initWebSocket: jest.fn(),
  registMethod: jest.fn(),
  unregistMethod: jest.fn(),
}));

test('renders app without crashing', () => {
  const { container } = render(<App />);
  expect(container).toBeTruthy();
});
