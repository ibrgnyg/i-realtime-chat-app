<template>
    <v-container>
        <v-row>
            <v-col cols="4">
                <menuComponent />
            </v-col>
            <v-col cols="5">
                <v-card variant="outlined">
                    <v-card-title>
                        <v-icon>mdi-security</v-icon> <s-page-header>Security</s-page-header>
                    </v-card-title>
                    <hr>
                    <v-card-text>
                        <v-expansion-panels>

                            <v-expansion-panel>
                                <v-expansion-panel-title>
                                    <template v-slot:default="{ expanded }">
                                        <v-row no-gutters>
                                            <v-col cols="4" class="d-flex align-center">
                                                <v-icon>mdi-form-textbox-password</v-icon>
                                                <span class="ml-2">Change Password</span>
                                            </v-col>
                                            <v-col class="text-grey" cols="8">
                                                <v-fade-transition leave-absolute>
                                                    <span>
                                                        Last change date :
                                                    </span>
                                                </v-fade-transition>
                                            </v-col>
                                        </v-row>
                                    </template>
                                </v-expansion-panel-title>
                                <v-expansion-panel-text>
                                    <v-row>
                                        <v-col cols="12">
                                            <v-text-field variant="outlined" v-model="model.oldPassword"
                                                placeholder="Old Password" hide-details></v-text-field>
                                        </v-col>
                                        <v-col cols="12">
                                            <v-text-field variant="outlined" v-model="model.newPassword"
                                                placeholder="New Password" hide-details></v-text-field>
                                        </v-col>
                                    </v-row>
                                    <v-row>
                                        <v-col cols="12" sm="6">
                                            <v-btn variant="outlined" :loading="loadingBtn"
                                                v-on:click="changePassword">Change Password</v-btn>
                                        </v-col>
                                    </v-row>
                                </v-expansion-panel-text>

                            </v-expansion-panel>

                        </v-expansion-panels>
                    </v-card-text>
                </v-card>
            </v-col>
        </v-row>
    </v-container>
</template>


<script setup>
import { ref } from 'vue';
import { useMutation } from '@vue/apollo-composable';
import { USER_MUTATION_UPDATE_PASSWORD } from '@/graphql/userQuery'
import { showToast } from '@/plugins/notificationHelper';
import authStore from '@/stores/authStore';
import menuComponent from '@/components/profile/Menu.vue'

const model = ref({
    id: authStore.getters.currentUser.user.id,
    oldPassword: '',
    newPassword: '',
});
const loadingBtn = ref(false);

const changePassword = () => {
    const { mutate: dtoModel } = useMutation(USER_MUTATION_UPDATE_PASSWORD);

    loadingBtn.value = true;
    dtoModel({
        id: model.value.id,
        oldPassword: model.value.oldPassword,
        newPassword: model.value.newPassword,
    }).then(result => {
        loadingBtn.value = false;
        const isError = result.data.updatePassword.isError;
        const message = result.data.updatePassword.message;

        if (isError) {
            showToast(message, 'error');
            return;
        }

        showToast(message);

    }).catch(error => {
        loadingBtn.value = false;
        showToast("error", 'error');
    });
}

</script>