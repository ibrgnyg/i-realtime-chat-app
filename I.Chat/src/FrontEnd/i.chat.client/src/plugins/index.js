import vuetify from './vuetify'
import router from '../router'
import { ApolloClient, InMemoryCache, createHttpLink } from '@apollo/client/core';
import { DefaultApolloClient, provideApolloClient } from '@vue/apollo-composable';
import { setContext } from '@apollo/client/link/context';
import ToastPlugin from 'vue-toast-notification';
import 'vue-toast-notification/dist/theme-bootstrap.css';
import authStore from '../stores/authStore';
import { connection, startConnection } from '@/plugins/signalRHelper'
import { VueSignalR } from '@dreamonkey/vue-signalr';
import moment from 'moment'

const httpLink = createHttpLink({
  uri: 'https://localhost:7163/gql',
});

const authLink = setContext((_, { headers }) => {
  const token = authStore.getters.getToken;
  const currentCulture = "en-US";
  const authHeader = token ? { 'Authorization': `Bearer ${token}` } : {};

  return {
    headers: {
      ...headers,
      'Accept-Language': currentCulture,
      ...authHeader,
    },
  };
});

const apolloClient = new ApolloClient({
  link: authLink.concat(httpLink),
  cache: new InMemoryCache(),
  defaultOptions: {
    watchQuery: {
      fetchPolicy: 'network-only'
    },
    query: {
      fetchPolicy: 'network-only'
    }
  }
});

provideApolloClient(apolloClient);

if (authStore.getters.isAuthenticated) {
  authStore.state.useRedirect = false;
  authStore.dispatch('updateUser');

  startConnection();
}

export function registerPlugins(app) {
  app
    .use(vuetify)
    .use(DefaultApolloClient)
    .use(router)
    .use(ToastPlugin)
    .use(authStore)
    .use(moment)
    .use(VueSignalR, { connection })


}