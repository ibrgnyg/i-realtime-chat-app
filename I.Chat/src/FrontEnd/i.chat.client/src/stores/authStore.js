import { createStore } from 'vuex';
import router from '../router'
import createPersistedState from 'vuex-persistedstate';
import SecureLS from 'secure-ls';
const ls = new SecureLS({ isCompression: false });
import { useQuery, useMutation } from '@vue/apollo-composable';
import { USER_QUERY_ID } from '@/graphql/userQuery';
import { USER_CONNECTIONID_MUTATION_UPDATE } from '@/graphql/userConnectionIdQuery';

export default createStore({
    state: {
        user: null,
        token: null,
        id: null,
        connectionId: null,
        isAuthenticated: false,
        useRedirect: false
    },
    mutations: {
        setUser(state, user) {
            state.user = user;
        },
        setToken(state, token) {
            state.token = token;
        },
        setAuthenticated(state, isAuthenticated) {
            state.isAuthenticated = isAuthenticated;
        },
        setRedirect(state, redirect) {
            state.useRedirect = redirect;
        },
        setConnectionId(state, id) {
            state.connectionId = id;
        },
        cleanState(state) {
            state.user = null;
            state.connectionId = null;
            state.token = null;
            state.id = null;
            state.isAuthenticated = false;
        },
    },
    actions: {
        login({ commit }, data) {
            this.state.id = data.id;
            const token = data.token;
            commit('setToken', token);
            commit('setRedirect', data.useRedirect);
            this.dispatch('updateUser');
        },

        updateConnectionId({ commit, state }, id) {
            const { mutate } = useMutation(USER_CONNECTIONID_MUTATION_UPDATE);

            const getUser = state.user;
            mutate({
                userId: getUser.user.id,
                connectionId: id,
            }).then(result => {

                const isError = result.data.updateUserConnectionId.isError;

                if (isError) {
                    commit('setConnectionId', 'error');
                    return;
                }

                commit('setConnectionId', id);
            }).catch(error => {
                commit('setConnectionId', 'error');
            });
        },

        updateUser({ commit, state }) {
            if (!state.id && !state.token) {
                commit('cleanState');
                return;
            }

            const { onResult, onError } = useQuery(USER_QUERY_ID, { id: state.id });
            onResult(queryResult => {
                if (!queryResult.loading) {
                    commit('setUser', queryResult.data);
                    if (state.useRedirect) {
                        window.location.href = "/profile";
                    }
                }
            })

            onError(error => {
                console.log(error);
                commit('cleanState');
                return;
            })
        },

        logout({ commit }) {
            commit('cleanState');
            router.push("/login");
        },
    },
    getters: {
        isAuthenticated: state => state.token != null && state.user != null,
        getToken: state => state.token,
        getUserId: state => state.id,
        currentUser(state) {
            return state.user;
        },
    },
    plugins: [
        createPersistedState({
            storage: {
                getItem: (key) => ls.get(key),
                setItem: (key, value) => ls.set(key, value),
                removeItem: (key) => ls.remove(key),
            },
            key: "ich_auth",
        }),
    ],
});
