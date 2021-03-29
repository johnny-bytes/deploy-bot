import { IconButton, TextField } from '@fluentui/react'
import React, { useRef, useState } from 'react'

interface FilePickerProps {
  onChange: (file: File) => void
  onGetErrorMessage?: () => string | JSX.Element | PromiseLike<string | JSX.Element> | undefined
}

export const FilePicker: React.FC<FilePickerProps> = (props) => {
  const fileRef = useRef<HTMLInputElement>()

  const [fileName, setFileName] = useState('')

  return (
    <React.Fragment>
      <TextField
        label="File"
        styles={{ suffix: { padding: 0 } }}
        readOnly={true}
        onRenderSuffix={() => (
          <IconButton onClick={() => fileRef.current?.click()} iconProps={{ iconName: 'FileRequest' }} styles={{ root: { height: '100%' } }} />
        )}
        value={fileName}
        onGetErrorMessage={props.onGetErrorMessage}
      />
      <input
        ref={fileRef}
        type="file"
        accept="application/zip"
        style={{ visibility: 'hidden' }}
        onChange={(ev) => {
          const files = ev.target.files
          const selectedFile = files.length === 1 ? files[0] : null

          setFileName(selectedFile?.name ?? '')
          props.onChange(selectedFile)
        }}
      />
    </React.Fragment>
  )
}
