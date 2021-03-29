import { DeploymentStatus } from './deployment.status.dto'

export interface Deployment {
  id: string
  version: string
  status: DeploymentStatus
  statusChangedOn: string
}
