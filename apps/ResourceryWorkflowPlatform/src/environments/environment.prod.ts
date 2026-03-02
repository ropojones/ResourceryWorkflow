import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'ResourceryWorkflow',
    logoUrl: '',
  },
  oAuthConfig: {
    issuer: 'https://localhost:7600/',
    redirectUri: baseUrl,
    postLogoutRedirectUri: 'http://localhost:4200/',
    clientId: 'ResourceryWorkflow_Platform',
    clientSecret: '1q2w3e*',
    responseType: 'code',
    scope: 'offline_access ResourceryWorkflowWorkflow ResourceryWorkflowWorkflow ResourceryWorkflowAuthServer ResourceryWorkflowIdentityService ResourceryWorkflowAdministration ResourceryWorkflowSaaS',
    requireHttps: true,
  },
  apis: {
    default: {
      url: 'https://localhost:7500',
      rootNamespace: 'ResourceryWorkflow',
    },
  },
} as Environment;
