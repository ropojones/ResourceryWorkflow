import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

export const environment = {
  production: false,
  application: {
    baseUrl,
    name: 'ResourceryWorkflow',
    logoUrl: '',
  },
  oAuthConfig: {
    issuer: 'https://localhost:7600/',
    redirectUri: baseUrl,
    clientId: 'ResourceryWorkflow_Platform',
    responseType: 'code',
    scope: 'ResourceryWorkflowWorkflow ResourceryWorkflowAuthServer ResourceryWorkflowIdentityService ResourceryWorkflowAdministration ResourceryWorkflowSaaS',
    requireHttps: false,
  },
  apis: {
    default: {
      url: 'https://localhost:7004',
      rootNamespace: 'ResourceryWorkflow',
    },
  },
  localization: {
    defaultResourceName: "Workflow",
  },
} as Environment;
