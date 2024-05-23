<template>
    <v-card class="mx-auto pa-2" max-width="300" variant="outlined">
        <v-list nav dense>
            <template v-for="(item, i) in menuItems" :key="i">
                <v-list-item :prepend-icon="item.icon" :to="item.path" :active="isActive(item.path)"
                    :color="isActive(item.path) ? 'orange' : ''">
                    {{ item.name }}
                </v-list-item>
            </template>
            <unredMessageComponent />
        </v-list>
        <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn variant="outlined" @click="logOut">Logout</v-btn>
        </v-card-actions>
    </v-card>

</template>

<script setup>
import { useRoute } from 'vue-router';
import { ref } from 'vue';
import unredMessageComponent from '@/components/message/UnreadMessageCount'
import authStore from '@/stores/authStore';

const menuItems = ref([
    { name: 'Profile', icon: 'mdi-account', path: '/profile', },
    { name: 'Security', icon: 'mdi-security', path: '/profile/security', },
]);

const route = useRoute();

const isActive = (path) => {
    return route.path === path;
};
const logOut = () => {
    authStore.dispatch("logout");
    window.location.href = "/";
}

</script>
