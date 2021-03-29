import { CommandButton, Dialog, DialogType, Spinner, Stack, Text, TextField } from '@fluentui/react'
import axios from 'axios'
import React from 'react'
import useSWR from 'swr'

import { DeploymentLogEntry } from '../../model/dto/deployment.log.entry'

export const ViewLogsDialog: React.FC<{ applicationId: string; deploymentId: string; onDismis: () => void }> = (props) => {
  const { data: logEntries, error } = useSWR<DeploymentLogEntry[]>(`api/applications/${props.applicationId}/deployments/${props.deploymentId}/logs`)

  const clearLogs = async () => {
    try {
      await axios.delete(`api/applications/${props.applicationId}/deployments/${props.deploymentId}/logs`)
    } finally {
      props.onDismis()
    }
  }

  return (
    <Dialog hidden={false} onDismiss={props.onDismis} minWidth="40vw" dialogContentProps={{ title: 'Logs', type: DialogType.largeHeader }}>
      {error != null && <Text>An unexpected error occurred while trying to load logs.</Text>}
      {error == null && logEntries == null ? <Spinner label="Loading..." /> : null}

      {logEntries != null && logEntries.length === 0 ? <Text>There are currently no logs available.</Text> : null}
      {logEntries != null
        ? logEntries.map((le) => (
            <TextField
              key={le.id}
              label={new Date(le.date).toLocaleString()}
              multiline
              readOnly
              value={le.logText}
              styles={{ field: { minHeight: '10rem' }, root: { marginBottom: '1.5rem' } }}
            />
          ))
        : null}
      {logEntries != null && logEntries.length > 0 && (
        <Stack horizontalAlign="end">
          <CommandButton iconProps={{ iconName: 'Clear' }} text="Clear" onClick={clearLogs} />
        </Stack>
      )}
    </Dialog>
  )
}
