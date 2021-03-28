import { CommandBar, DetailsList, ICommandBarItemProps, SelectionMode, Spinner, Stack } from '@fluentui/react'
import React from 'react'
import useSWR from 'swr'

import { Product, Release } from '../../model/dto'

const _items: ICommandBarItemProps[] = [
  {
    key: 'newRelease',
    text: 'New release',
    iconProps: { iconName: 'Add' },
  },
  {
    key: 'deployRelease',
    text: 'Deploy release',
    iconProps: { iconName: 'Deploy' },
  },
]

interface ReleaseListProps {
  product: Product
}

export const ReleaseList: React.FC<ReleaseListProps> = (props) => {
  const { data, error } = useSWR<Release[]>(`api/releases/${props.product.name}`)

  if (error != null) return <div>An unexpected error occurred.</div>

  if (data == null) return <Spinner label="Loading..." />

  return (
    <Stack style={{ marginTop: '1rem' }}>
      <Stack.Item>
        <Stack horizontal>
          <Stack.Item>
            <h2>{props.product.name}</h2>
          </Stack.Item>
          <Stack.Item grow>
            <CommandBar items={[]} farItems={_items} />
          </Stack.Item>
        </Stack>
      </Stack.Item>

      <DetailsList
        columns={[{ key: 'release.table.version', name: 'Version', minWidth: 150, fieldName: 'version' }]}
        items={data}
        selectionMode={SelectionMode.single}
      />
    </Stack>
  )
}
