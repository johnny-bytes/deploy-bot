import { PartialTheme, ThemeProvider } from '@fluentui/react'
import { initializeIcons } from '@uifabric/icons'
import React from 'react'
import ReactDOM from 'react-dom'
import { HashRouter as Router } from 'react-router-dom'
import { SWRConfig } from 'swr'

import { MainLayout } from './components/main-layout/main.layout.component'

initializeIcons()

const partialTheme: PartialTheme = {
  palette: {
    themePrimary: '#f71977',
    themeLighterAlt: '#0a0105',
    themeLighter: '#280413',
    themeLight: '#4a0723',
    themeTertiary: '#940f46',
    themeSecondary: '#da1667',
    themeDarkAlt: '#f82f83',
    themeDark: '#f94f96',
    themeDarker: '#fb7cb1',
    neutralLighterAlt: '#2b022b',
    neutralLighter: '#330433',
    neutralLight: '#410941',
    neutralQuaternaryAlt: '#4a0e4a',
    neutralQuaternary: '#511251',
    neutralTertiaryAlt: '#6f276f',
    neutralTertiary: '#c8c8c8',
    neutralSecondary: '#d0d0d0',
    neutralPrimaryAlt: '#dadada',
    neutralPrimary: '#ffffff',
    neutralDark: '#f4f4f4',
    black: '#f8f8f8',
    white: '#220022',
  },
}

const fetcher = async (input: RequestInfo, init: RequestInit, ...args: any[]) => {
  const res = await fetch(input, init)
  return res.json()
}

ReactDOM.render(
  <SWRConfig
    value={{
      refreshInterval: 3000,
      fetcher,
    }}
  >
    <ThemeProvider applyTo="body" theme={partialTheme}>
      <Router>
        <MainLayout />
      </Router>
    </ThemeProvider>
  </SWRConfig>,
  document.getElementById('root'),
)
