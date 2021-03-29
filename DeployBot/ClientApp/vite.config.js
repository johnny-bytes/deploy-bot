import path from 'path'

const config = {
  // eslint-disable-next-line no-undef
  root: path.join(__dirname, 'src'),
  base: './',
  server: {
    strictPort: true,
    hmr: {
      protocol: 'ws',
    },
  },
  build: {
    // eslint-disable-next-line no-undef
    outDir: path.join(__dirname, '..', 'wwwroot'),
  },
}

export default config
