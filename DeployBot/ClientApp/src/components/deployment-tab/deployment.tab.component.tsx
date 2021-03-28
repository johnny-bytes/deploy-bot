import React from 'react'

import { Product } from '../../model/dto'
import { ReleaseList } from '../release-list/release.list.component'

interface ReleaseTabProps {
  products: Product[]
}

const DeploymentTab: React.FC<ReleaseTabProps> = (props) => {
  return (
    <React.Fragment>
      {props.products.map((product) => (
        <div key={product.name}>
          <h2>{product.name}</h2>

          <ReleaseList product={product} />
        </div>
      ))}
    </React.Fragment>
  )
}

export default DeploymentTab
