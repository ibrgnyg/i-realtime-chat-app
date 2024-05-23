<template>
    <v-card variant="outlined" max-width="1000" v-if="props.selected">
        <v-card-title>
            <v-list-item class="w-100">
                <template v-slot:prepend v-if="props.selectedMessage != null">
                    <v-avatar color="grey-darken-3"  size="45" @click="navigateToChannel" :image="props.selectedMessage.avatar">
                    </v-avatar>
                </template>
                <v-list-item-title @click="navigateToChannel(selectedMessage.name)">
                    {{ selectedMessage.name }}
                </v-list-item-title>
                <!-- 
                <template v-slot:append v-if="!props.isNew">
                    <v-menu>
                        <template v-slot:activator="{ props }">
                            <v-btn variant="text" icon="mdi-dots-vertical" v-bind="props"></v-btn>
                        </template>
                        <v-list lines="one">
                      
                            <v-list-item>
                                <v-btn variant="outlined" disabled>Delete</v-btn>
                            </v-list-item>
                            <v-list-item>
                                <v-btn variant="outlined" @click="blockMessage">
                                    {{ blockedInputModel.blocked ? "Un BlockUser" : "Block User" }}
                                </v-btn>
                            </v-list-item>
                        </v-list>
                    </v-menu>
                </template>-->
            </v-list-item>
        </v-card-title>
        <v-divider></v-divider>
        <v-container>
            <v-infinite-scroll height="450" side="start" @load="load" mode="manual">
                <div v-for="(item, index) in contentMessages" :key="index">
                    <v-col :class="{ 'd-flex flex-row-reverse': authUserId == item.userId }" cols="12">
                        <v-card max-width="300" max-height="120"
                            :color="authUserId == item.userId ? 'orange' : 'primary'" outlined class="pa-4 mb-2">
                            <v-card-text>
                                <span class="font-weight-bold">{{ item.message }}</span>
                                <br>
                                <span class="caption grey--text">{{ formatDateRelative(item.createDate) }}</span>
                            </v-card-text>
                        </v-card>
                    </v-col>
                </div>
                <template v-slot:empty>
                    <v-alert type="warning">No more items!</v-alert>
                </template>
            </v-infinite-scroll>
        </v-container>
        <v-container>
            <v-row>
                <v-divider></v-divider>
                <v-col cols="12" v-if="!blockedInputModel.blocked">
                    <v-textarea v-model="messageSendModel.messageContent" label="Type a message" variant="outlined">
                    </v-textarea>
                    <v-btn @click="sendMessage" :loading="loadingBtn" variant="outlined">Send</v-btn>
                </v-col>
                <v-col cols="12" v-else>
                    <div class="text-center">
                        Have you blocked this person?
                    </div>
                </v-col>
            </v-row>
        </v-container>
    </v-card>
</template>

<script setup>
import { ref, defineProps, reactive, watch, onMounted, } from 'vue';
import { useQuery } from '@vue/apollo-composable';
import { MESSAGE_QUERY_ID } from '@/graphql/messageQuery';
import authStore from '@/stores/authStore';
import { connection } from "@/plugins/signalrHelper";
import { showToast } from '@/plugins/notificationHelper';
import { formatDateRelative } from '@/plugins/momentHelper';

const defaultUser = ref('https://www.tenforums.com/attachments/user-accounts-family-safety/322690d1615743307-user-account-image-log-user.png');
const loadingBtn = ref(false);
const props = defineProps(['selected', 'isNew', 'selectedMessage']);
const authUserId = ref(authStore.getters.getUserId);
const currentPage = ref(1);

const blockedInputModel = ref({
    userId: '',
    blocked: false
});

const messageSendModel = reactive({
    id: '',
    startUserId: '',
    toUserId: '',
    messageContent: ''
});
const contentMessages = ref([]);

const stopRequest = ref(false);

const Id = ref('');

const sendMessage = () => {

    if (messageSendModel.messageContent == "" || messageSendModel.messageContent == null)
        return;

    loadingBtn.value = true;

    messageSendModel.id = props.selectedMessage.id;
    messageSendModel.startUserId = authUserId;
    messageSendModel.toUserId = props.selectedMessage.userId;

    connection.invoke("SendMessage", messageSendModel).then(result => {
        if (result.isError) {
            showToast("error", "error")
            loadingBtn.value = false;
            return;
        }
        console.log("SendMessage result:", result);

        loadingBtn.value = false;
        messageSendModel.messageContent = "";
        if (messageSendModel.id == "" || messageSendModel.id == null) {
            messageSendModel.id = result.fields.Id;
            props.selectedMessage.id = result.fields.Id;
        }
        contentMessages.value.push({
            message: result.fields.Model.messageContent,
            userId: result.fields.Model.startUserId
        });

        connection.invoke('GetMessageList', authUserId.value)

    }).catch(error => {
        console.error("SendMessage result error!:", error);
        showToast("error", "error")
        loadingBtn.value = false;
    });
};

connection.on("ReceiveMessage", (receiveMessage) => {
    contentMessages.value.push({
        message: receiveMessage.fields.Model.messageContent,
        userId: receiveMessage.fields.Model.startUserId
    });
})

const load = async ({ done }) => {
    if (stopRequest.value) {
        done('empty');
        return;
    }

    const { onResult } = await useQuery(MESSAGE_QUERY_ID, {
        id: Id.value,
        userId: authUserId,
        activePage: currentPage.value,
        pageSize: 5,
    });

    onResult(queryResult => {
        if (!queryResult.loading) {
            if (queryResult.data.message.messages.length > 0) {

                queryResult.data.message.messages.forEach(item => {
                    if (item && typeof item === 'object' && '__typename' in item) {
                        const { __typename, ...rest } = item;

                        if (!contentMessages.value.some(message => message.id === rest.id)) {
                            contentMessages.value.push(rest);
                        }
                    }
                });

                contentMessages.value.sort((a, b) => new Date(a.createDate) - new Date(b.createDate));

                currentPage.value++;
                done('ok');
            } else {
                done('empty');
                stopRequest.value = true;
            }
        }
    })
};

watch(
    () => props.selectedMessage.id,
    (newId, oldId) => {
        if (newId !== oldId) {
            Id.value = newId;
            contentMessages.value = [];
            currentPage.value = 1;
            stopRequest.value = false;
            load({ done: () => { } });
        }
    }
);


const blockMessage = () => {
    const blockModel = {
        Id: props.selectedMessage.id,
        StartUserId: authUserId.value,
        ToUserId: props.selectedMessage.userId,
    }
    connection.invoke("BlockMessage", blockModel).then(result => {

        blokedInput.value = true;

    }).catch(error => {

    });
}
</script>