import React from 'react';
import { render, screen } from '@testing-library/react';
import App from './App';

test('renders Bugs link', () => {
  render(<App />);
  const linkElement = screen.getByText(/Bugs/i);
  expect(linkElement).toBeInTheDocument();
});
