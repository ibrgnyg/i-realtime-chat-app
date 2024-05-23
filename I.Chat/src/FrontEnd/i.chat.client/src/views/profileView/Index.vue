<template>
    <v-container>
        <v-row>
            <v-col cols="4">
                <menuComponent />
            </v-col>
            <v-col cols="5">

                <v-card variant="outlined">
                    <v-card-title>
                        <v-icon>mdi-account</v-icon> <s-page-header>Account</s-page-header>
                    </v-card-title>
                    <hr>
                    <v-card-text>
                        <v-row>
                            <v-col cols="12">
                                <v-text-field label="username" v-model="model.userName" variant="outlined"
                                    required></v-text-field>
                            </v-col>
                            <v-spacer></v-spacer>
                            <v-col cols="12" sm="6">
                                <v-btn variant="outlined" :loading="loadingBtn" @click="changeName">Change</v-btn>
                            </v-col>
                        </v-row>
                    </v-card-text>
                </v-card>
            </v-col>
        </v-row>
    </v-container>
</template>

<script setup>
import { ref } from 'vue';
import { useMutation } from '@vue/apollo-composable';
import { USER_MUTATION_UPDATE_USERNAME } from '@/graphql/userQuery'
import { showToast } from '@/plugins/notificationHelper';
import authStore from '@/stores/authStore';
import menuComponent from '@/components/profile/Menu.vue';

const model = ref({
    id: authStore.getters.currentUser.user.id,
    userName: authStore.getters.currentUser.user.userName,
});
const loadingBtn = ref(false);

const { mutate: dtoModel } = useMutation(USER_MUTATION_UPDATE_USERNAME);

const changeName = () => {
    loadingBtn.value = true;
    dtoModel({
        id: model.value.id,
        userName: model.value.userName,
    }).then(result => {
        loadingBtn.value = false;
        const isError = result.data.updateUserName.isError;
        const message = result.data.updateUserName.message;

        if (isError) {
            showToast(message, 'error');
            return;
        }

        authStore.state.useRedirect = false;
        authStore.dispatch('updateUser');
        showToast(message);
    }).catch(error => {
        loadingBtn.value = false;
        showToast("error", 'error');
    });
}

</script>