<template>
    <v-container fluid>
        <v-row>
            <v-col cols="2">
                <menuComponent />
            </v-col>
            <v-col cols="3">
                <v-card variant="outlined" class="mx-auto" max-width="800">
                    <v-card-title>
                        Messages
                    </v-card-title>
                    <v-container>
                        <v-btn variant="outlined" @click="dialog = true"> New Create Message</v-btn>
                    </v-container>

                    <v-dialog width="500" persistent v-model="dialog">
                        <v-card title="New Message">
                            <v-card-text>
                                <v-text-field density="compact" variant="outlined" label="Search" v-model="searchTerm"
                                    append-inner-icon="mdi-magnify" @keyup.enter="userSearch" single-line
                                    hide-details></v-text-field>
                            </v-card-text>
                            <v-list lines="two" dense class="overflow-y-auto">
                                <v-list-item v-for="item in users" :key="item.userId" :title="item.name"
                                    :prepend-avatar="item.avatar"
                                    v-on:click="selectMessage(item, item.id, true)"></v-list-item>
                            </v-list>
                            <div class="text-center" v-if="users.length == 0">
                                Not found result!
                            </div>
                            <v-card-actions>
                                <v-spacer></v-spacer>
                                <v-btn variant="outlined" text="Close" @click="dialog = false"></v-btn>
                            </v-card-actions>
                        </v-card>
                    </v-dialog>

                    <v-divider></v-divider>
                    <v-list lines="two" dense style="max-height: 1100px" class="overflow-y-auto">
                        <v-list-item v-for="item in messages" :key="item.id" :title="item.name"
                            :subtitle="item.lastMessage" v-on:click="selectMessage(item, item.id, false)"
                            :prepend-avatar="item.avatar" :class="{ 'selected-item': item.isSelected }">
                            <template v-slot:append>
                                <v-btn size="x-small" color="blue" icon="$vuetify" variant="outlined"
                                    v-if="item.unreadMessageCount > 0">
                                    {{ item.unreadMessageCount }}
                                </v-btn>
                            </template>
                        </v-list-item>
                    </v-list>
                    <div class="text-center" v-if="messages.length == 0">
                        Not found messages :(
                    </div>
                </v-card>
            </v-col>
            <v-col cols="7">
                <messageDetail :selected="showMessageDetail" :isNew="isNewMessage" :selectedMessage="selectedMessage" />
            </v-col>
        </v-row>
    </v-container>
</template>

<script setup>

import { ref, onMounted, watch } from 'vue';
import authStore from '@/stores/authStore';
import messageDetail from '@/components/message/Work.vue'
import { connection } from "@/plugins/signalrHelper";
import { useQuery } from '@vue/apollo-composable';
import { USER_QUERY_SEARCH } from '@/graphql/userQuery';
import menuComponent from '@/components/profile/Menu.vue';

const searchTerm = ref('');
const showMessageDetail = ref(false);
const dialog = ref(false);
const isNewMessage = ref(false);
const authUserId = ref(authStore.getters.getUserId);
const users = ref([]);
const selectedMessage = ref({});
const messages = ref([]);

const getMessages = () => {
    connection.invoke('GetMessageList', authUserId.value);
}

connection.on("GetMessages", (resultMessages) => {
    messages.value = resultMessages;
});

const selectMessage = async (item, id, isNewType) => {
    if (!isNewType && selectedMessage.value.id == id) {
        return;
    }

    messages.value.forEach(item => {
        item.isSelected = item.id === id;

        if (item.unreadMessageCount > 0) {
            item.unreadMessageCount = 0;
        }
    });

    selectedMessage.value = {
        id: id,
        userId: item.userId,
        name: isNewType ? item.name : item.name,
        avatar: item.avatar,
    };

    isNewMessage.value = isNewType;
    showMessageDetail.value = true;
    dialog.value = false;

    const model = {
        Id: selectedMessage.value.id,
        UserId: authUserId.value
    };

    await connection.invoke("JoinConversation", model).catch(err => console.error(err.toString()));
    await connection.invoke("UpdateMessageIsRead", model).catch(err => console.error(err.toString()));
}

const userSearch = () => {
    const { onResult, onError } = useQuery(USER_QUERY_SEARCH, { userName: searchTerm.value, userId: authUserId.value });

    onResult(queryResult => {
        if (!queryResult.loading) {
            users.value = queryResult.data.searchUsers;
            return;
        }
    })

    onError(error => {
        console.log(error);
        return;
    })
}

onMounted(async () => {
    getMessages();
});

</script>

<style scoped>
.selected-item {
    background-color: #FFA21A
}
</style>
