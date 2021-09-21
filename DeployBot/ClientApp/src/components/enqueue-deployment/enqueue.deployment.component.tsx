import { CommandButton, MessageBar, MessageBarType, Stack } from '@fluentui/react'
import axios from 'axios'
import React, { useState } from 'react'

export const EnqueueDeployment: React.FC<{ applicationId: string; deploymentId: string }> = (props) => {
  const [showError, setShowError] = useState(false)

  const enqueueDeployment = async () => {
    try {
      await axios.post(`api/applications/${props.applicationId}/deployments/${props.deploymentId}/enqueue`)
    } catch (err) {
      console.error(err)
      setShowError(true)
    }
  }

  return (
    <React.Fragment>
      {showError && (
        <Stack horizontalAlign="center" styles={{ root: { position: 'fixed', right: '1rem', bottom: '1rem', left: '1rem' } }}>
          <Stack.Item styles={{ root: { maxWidth: '1024px', width: '100%' } }}>
            <MessageBar
              messageBarType={MessageBarType.error}
              isMultiline={false}
              onDismiss={() => setShowError(false)}
              dismissButtonAriaLabel="Close"
            >
              An unexpected error occurred while trying to enqueue deployment.
            </MessageBar>
          </Stack.Item>
        </Stack>
      )}

      <CommandButton iconProps={{ iconName: 'BuildQueueNew' }} text="Enqueue" onClick={() => enqueueDeployment()} />
    </React.Fragment>
  )
}
