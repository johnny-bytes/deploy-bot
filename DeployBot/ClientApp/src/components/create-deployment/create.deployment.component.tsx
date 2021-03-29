import { CommandButton, DefaultButton, Dialog, DialogFooter, DialogType, Overlay, PrimaryButton, Spinner, Text, TextField } from '@fluentui/react'
import axios from 'axios'
import React, { useState } from 'react'

import { FilePicker } from '../file-picker/file.picker.component'

export const CreateDeployment: React.FC<{ applicationId: string }> = (props) => {
  const [showCreateModal, setShowCreateModal] = useState(false)
  const [version, setVersion] = useState('')
  const [file, setFile] = useState<File | null>(null)

  const [isCreating, setIsCreating] = useState(false)
  const [createError, setCreateError] = useState<string | null>(null)

  const closeDialog = () => {
    setShowCreateModal(false)

    setVersion('')
    setCreateError(null)
  }

  const createDeployment = async () => {
    try {
      if (!validateFile() || !validateVersion()) {
        return
      }

      setIsCreating(true)
      setCreateError(null)

      const formData = new FormData()
      formData.append('version', version)
      formData.append('release_zip', file)

      await axios.post(`api/applications/${props.applicationId}/deployments`, formData, {
        headers: {
          'content-type': 'multipart/form-data',
        },
      })

      closeDialog()
    } catch (err) {
      setCreateError('An unexpected error occurred while trying to create application.')
    } finally {
      setIsCreating(false)
    }
  }

  const validateVersion = (): boolean => {
    return version.length > 0
  }

  const validateFile = (): boolean => {
    return file != null
  }

  return (
    <React.Fragment>
      <CommandButton iconProps={{ iconName: 'Add' }} text="New" onClick={() => setShowCreateModal(true)} />
      <Dialog hidden={!showCreateModal} onDismiss={closeDialog} dialogContentProps={{ title: 'New Deployment', type: DialogType.largeHeader }}>
        <TextField
          label="Version"
          value={version}
          onChange={(_, newValue) => setVersion(newValue)}
          required
          onGetErrorMessage={() => (!validateVersion() ? 'Version cannot be empty.' : undefined)}
        />

        <FilePicker onChange={(file) => setFile(file)} onGetErrorMessage={() => (!validateFile() ? 'Please select a file.' : undefined)} />
        <DialogFooter>
          <PrimaryButton onClick={createDeployment} text="Create" disabled={!(validateFile() && validateVersion())} />
          <DefaultButton onClick={closeDialog} text="Cancel" />

          {createError != null ? (
            <div>
              <Text>{createError}</Text>
            </div>
          ) : null}
        </DialogFooter>
        {isCreating && (
          <Overlay>
            <Spinner label="Creating deployment..." />
          </Overlay>
        )}
      </Dialog>
    </React.Fragment>
  )
}
