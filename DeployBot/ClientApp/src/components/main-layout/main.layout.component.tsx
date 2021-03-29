import { Pivot, PivotItem, Stack } from '@fluentui/react'
import React, { useEffect, useState } from 'react'
import { Route, Switch, useHistory, useLocation } from 'react-router-dom'
import useSWR from 'swr'

import { Application } from '../../model/dto'
import { CreateApplication } from '../create-application/create.application.component'
import { DeploymentList } from '../deployment-list/deployment.list.component'
import { mainStyles, pivotStyle } from './main.layout.styles'

export const MainLayout: React.FC = () => {
  const { data, error } = useSWR<Application[]>(`/api/applications`)
  const [selectedApplication, setSelectedApplication] = useState<Application | null>(null)

  const history = useHistory()

  useEffect(() => {
    if (data != null && data.length > 0 && selectedApplication == null) {
      setSelectedApplication(data[0])
    }
  }, [data, selectedApplication])

  if (error != null) {
    return <div>An unexpected error occurred.</div>
  }

  if (data == null) {
    return <div>Loading...</div>
  }

  return (
    <React.Fragment>
      <Stack className={mainStyles.navigation} horizontal verticalAlign="center" tokens={{ childrenGap: '1rem', padding: '0 1rem' }}>
        <Stack.Item>
          <img className={mainStyles.logo} src="https://www.johnnybytes.com/static/logo-white-25fad89acedc918d44bc12eeaa360c6b.svg" />
        </Stack.Item>
        <Stack.Item>
          <Pivot styles={pivotStyle} selectedKey={selectedApplication?.id} onLinkClick={(link) => history.push(link?.props.itemKey ?? '')}>
            {data.map((app) => (
              <PivotItem key={app.id} headerText={app.name} />
            ))}
          </Pivot>
        </Stack.Item>
        <Stack.Item grow={1}>
          <div />
        </Stack.Item>
        <Stack.Item>
          <CreateApplication />
        </Stack.Item>
      </Stack>

      <Stack horizontalAlign="center">
        <Stack.Item styles={{ root: { maxWidth: '1024px', width: '100%' } }}>
          {selectedApplication && <DeploymentList applicationId={selectedApplication.id} />}
        </Stack.Item>
      </Stack>
    </React.Fragment>
  )
}
