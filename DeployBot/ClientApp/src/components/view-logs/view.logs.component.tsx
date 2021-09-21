import { CommandButton } from '@fluentui/react'
import React, { useState } from 'react'

import { ViewLogsDialog } from './view.logs.dialog.component'

export const ViewLogs: React.FC<{ applicationId: string; deploymentId: string }> = (props) => {
  const [showLogsModal, setShowLogsModal] = useState(false)

  const closeDialog = () => {
    setShowLogsModal(false)
  }

  return (
    <React.Fragment>
      <CommandButton iconProps={{ iconName: 'EntryView' }} text="Logs" onClick={() => setShowLogsModal(true)} />
      {showLogsModal && <ViewLogsDialog applicationId={props.applicationId} deploymentId={props.deploymentId} onDismis={closeDialog} />}
    </React.Fragment>
  )
}
