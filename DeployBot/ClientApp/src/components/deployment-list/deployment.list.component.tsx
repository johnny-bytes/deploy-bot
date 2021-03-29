import { DetailsList, IColumn, Selection, SelectionMode, Spinner, Stack, Text } from '@fluentui/react'
import React, { useState } from 'react'
import useSWR from 'swr'

import { Deployment, DeploymentStatus } from '../../model/dto'
import { CreateDeployment } from '../create-deployment/create.deployment.component'
import { EnqueueDeployment } from '../enqueue-deployment/enqueue.deployment.component'
import { RemoveDeployment } from '../remove-deployment/remove.deployment.component'
import { ViewLogs } from '../view-logs/view.logs.component'

const renderDateColumn = (item: Deployment) => {
  return new Date(item.statusChangedOn).toLocaleString()
}

const renderStatusColumn = (item: Deployment) => {
  switch (item.status) {
    case DeploymentStatus.enqueued:
      return 'Enqueued'
    case DeploymentStatus.failed:
      return 'Failed'
    case DeploymentStatus.inProgress:
      return 'In Progress'
    case DeploymentStatus.pending:
      return 'Pending'
    case DeploymentStatus.success:
      return 'Deployed'
  }
}

const DeploymentListColumns: IColumn[] = [
  { key: 'release.table.version', name: 'Version', minWidth: 150, fieldName: 'version' },
  { key: 'release.table.date', name: 'Status Changed On', minWidth: 150, fieldName: 'statusChangedOn', onRender: renderDateColumn },
  { key: 'release.table.status', name: 'Status', minWidth: 150, fieldName: 'status', onRender: renderStatusColumn },
]

export const DeploymentList: React.FC<{ applicationId: string }> = (props) => {
  const [selectedDeployment, setSelectedDeployment] = useState<Deployment | null>(null)

  const { data, error } = useSWR<Deployment[]>(`api/applications/${props.applicationId}/deployments`)

  if (error != null) return <div>An unexpected error occurred.</div>

  if (data == null) return <Spinner label="Loading..." style={{ paddingTop: '10%' }} />

  const _selection = new Selection({
    onSelectionChanged: () => {
      if (_selection.count === 1) {
        setSelectedDeployment(_selection.getItems()[0] as Deployment)
      } else {
        setSelectedDeployment(null)
      }
    },
  })

  return (
    <Stack style={{ marginTop: '1rem' }} verticalAlign="center">
      <Stack.Item>
        <Stack horizontal verticalAlign="center">
          <Text variant="large">Deployments</Text>

          <Stack.Item grow> </Stack.Item>

          <CreateDeployment applicationId={props.applicationId} />
          {selectedDeployment != null && <EnqueueDeployment applicationId={props.applicationId} deploymentId={selectedDeployment.id} />}
          {selectedDeployment != null && <ViewLogs applicationId={props.applicationId} deploymentId={selectedDeployment.id} />}
          {selectedDeployment != null && <RemoveDeployment applicationId={props.applicationId} deploymentId={selectedDeployment.id} />}
        </Stack>
      </Stack.Item>

      {data.length > 0 ? (
        <DetailsList columns={DeploymentListColumns} items={data} selectionMode={SelectionMode.single} selection={_selection} />
      ) : (
        <Stack horizontalAlign="center">
          <Text variant="large">There are currently no deployments.</Text>
        </Stack>
      )}
    </Stack>
  )
}
