import { CommandButton, MessageBar, MessageBarType, Stack } from '@fluentui/react'
import axios from 'axios'
import React, { useState } from 'react'

export const RemoveDeployment: React.FC<{ applicationId: string; deploymentId: string }> = (props) => {
  const [showError, setShowError] = useState(false)

  const removeDeployment = async () => {
    try {
      await axios.delete(`api/applications/${props.applicationId}/deployments/${props.deploymentId}`)
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
              An unexpected error occurred while trying to delete deployment.
            </MessageBar>
          </Stack.Item>
        </Stack>
      )}

      <CommandButton iconProps={{ iconName: 'Trash' }} text="Remove" onClick={() => removeDeployment()} />
    </React.Fragment>
  )
}
