import path from 'path'

const config = {
  // eslint-disable-next-line no-undef
  root: path.join(__dirname, 'src'),
  base: '/',
  server: {
    strictPort: true,
    hmr: {
      protocol: 'ws',
    },
  },
}

export default config
