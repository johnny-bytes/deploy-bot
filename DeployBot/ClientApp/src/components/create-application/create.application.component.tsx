import { CommandButton, DefaultButton, Dialog, DialogFooter, DialogType, Overlay, PrimaryButton, Spinner, Text, TextField } from '@fluentui/react'
import axios from 'axios'
import React, { useState } from 'react'

export const CreateApplication: React.FC = () => {
  const [showCreateModal, setShowCreateModal] = useState(false)
  const [appName, setAppName] = useState('')

  const [isCreating, setIsCreating] = useState(false)
  const [createError, setCreateError] = useState<string | null>(null)

  const closeDialog = () => {
    setShowCreateModal(false)

    setAppName('')
    setCreateError(null)
  }

  const createApplication = async () => {
    try {
      setIsCreating(true)
      setCreateError(null)
      await axios.post('api/applications', { name: appName })

      closeDialog()
    } catch (err) {
      setCreateError('An unexpected error occurred while trying to create application.')
    } finally {
      setIsCreating(false)
    }
  }

  const validateName = () => {
    return appName.length >= 3
  }

  return (
    <React.Fragment>
      <CommandButton iconProps={{ iconName: 'Add' }} text="New Application" onClick={() => setShowCreateModal(true)} />
      <Dialog hidden={!showCreateModal} onDismiss={closeDialog} dialogContentProps={{ title: 'Create application', type: DialogType.largeHeader }}>
        <TextField
          label="Name"
          value={appName}
          onChange={(_, newValue) => setAppName(newValue)}
          required
          onGetErrorMessage={() => (!validateName() ? 'Application name must be at least 3 character long.' : undefined)}
        />
        <DialogFooter>
          <PrimaryButton onClick={createApplication} text="Create" disabled={!validateName()} />
          <DefaultButton onClick={closeDialog} text="Cancel" />

          {createError != null ? (
            <div>
              <Text>{createError}</Text>
            </div>
          ) : null}
        </DialogFooter>
        {isCreating && (
          <Overlay>
            <Spinner label="Creating application..." />
          </Overlay>
        )}
      </Dialog>
    </React.Fragment>
  )
}
