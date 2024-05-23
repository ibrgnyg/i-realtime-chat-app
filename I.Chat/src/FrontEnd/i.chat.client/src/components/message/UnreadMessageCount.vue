<template>
    <v-list-item prepend-icon="mdi-message" to="/messages"  :color="isActive('/messages') ? 'orange' : ''">
        Messages
        <v-btn size="x-small" color="blue" icon="$vuetify" variant="outlined" v-if="unreadMessageCount > 0">
            {{ unreadMessageCount }}
        </v-btn>
    </v-list-item>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import authStore from '@/stores/authStore';
import { connection } from "@/plugins/signalrHelper";
import { useRoute } from 'vue-router';

const unreadMessageCount = ref(0);
const authUserId = ref(authStore.getters.getUserId);

const getUnreadMessageCount = () => {

    connection.invoke('GetTotalUnreadMessageCount', authUserId.value)
        .then(messages => {
            console.log("Received Unread Messages:", messages);
            unreadMessageCount.value = messages;
        })
        .catch(error => {
            console.error('Error getting unread messages count:', error);
        });
}

connection.on("GetMessageCount", (count) => {
    unreadMessageCount.value = count;
});

onMounted(async () => {
    getUnreadMessageCount();
});

const route = useRoute();

const isActive = (path) => {
    return route.path === path;
};


</script>