import { DeploymentStatus } from './deployment.status.dto'

export interface Deployment {
  version: string
  status: DeploymentStatus
  statusChangedOn: string
}
