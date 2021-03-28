import { IconButton, Pivot, PivotItem, Spinner, Stack } from '@fluentui/react'
import React, { Suspense, useEffect, useState } from 'react'
import { Redirect, Route, Switch, useHistory, useLocation } from 'react-router-dom'
import useSWR from 'swr'

import { Product } from '../../model/dto'
import { buttonStyle, mainStyles, pivotStyle } from './main.layout.styles'

const DeploymentTab = React.lazy(() => import('../deployment-tab/deployment.tab.component'))
const ReleaseTab = React.lazy(() => import('../release-tab/release.tab.component'))

export const MainLayout: React.FC = () => {
  const { data, error } = useSWR<Product[]>(`/api/products`)
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null)

  const history = useHistory()
  const location = useLocation()

  useEffect(() => {
    if (data != null && data.length > 0 && selectedProduct == null) {
      setSelectedProduct(data[0])
    }
  }, [data, selectedProduct])

  if (error != null) {
    return <div>An unexpected error occurred.</div>
  }

  if (data == null) {
    return <div>Loading...</div>
  }

  if (selectedProduct == null) {
    return <div>No products.</div>
  }

  return (
    <React.Fragment>
      <Stack className={mainStyles.navigation} horizontal verticalAlign="center" tokens={{ childrenGap: '1rem', padding: '0 1rem' }}>
        <Stack.Item>
          <img className={mainStyles.logo} src="https://www.johnnybytes.com/static/logo-white-25fad89acedc918d44bc12eeaa360c6b.svg" />
        </Stack.Item>
        <Stack.Item>
          <Pivot styles={pivotStyle} selectedKey={location.pathname} onLinkClick={(link) => history.push(link?.props.itemKey ?? '')}>
            <PivotItem headerText="Releases" itemKey={'/releases'} />
            <PivotItem headerText="Deployments" itemKey={'/deployments'} />
          </Pivot>
        </Stack.Item>
        <Stack.Item grow={1}>
          <div />
        </Stack.Item>
        <Stack.Item>
          <IconButton styles={buttonStyle} iconProps={{ iconName: 'Settings' }} title="Settings" />
        </Stack.Item>
      </Stack>

      <Stack horizontalAlign="center">
        <Stack.Item styles={{ root: { maxWidth: '1024px', width: '100%' } }}>
          <Switch>
            <Route exact path="/">
              <Redirect to="/releases" />
            </Route>
            <Route exact path="/releases">
              <Suspense fallback={<Spinner label="Loading..." style={{ paddingTop: '10%' }} />}>
                <ReleaseTab products={data} />
              </Suspense>
            </Route>
            <Route exact path="/deployments">
              <Suspense fallback={<Spinner label="Loading..." style={{ paddingTop: '10%' }} />}>
                <DeploymentTab products={data} />
              </Suspense>
            </Route>
            <Route exact path="/settings">
              <div>Einstellungen</div>
            </Route>
          </Switch>
        </Stack.Item>
      </Stack>
    </React.Fragment>
  )
}
