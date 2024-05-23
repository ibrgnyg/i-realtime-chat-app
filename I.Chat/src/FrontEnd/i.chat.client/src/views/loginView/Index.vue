<template>
    <v-col class="text-center">
        <v-dialog v-model="dialog" persistent max-width="500px" height="500">
            <v-card>
                <v-card-title> i-chat | Login</v-card-title>
                <v-divider></v-divider>
                <v-card-text>
                    <v-text-field v-model="model.email" label="E-Posta" variant="outlined" type="email"
                        required></v-text-field>
                    <v-text-field v-model="model.password" label="Şifre" variant="outlined" type="password" required
                        outlined></v-text-field>
                    <v-btn variant="outlined" :loading="loadingBtn" v-on:click="login">Login</v-btn>
                </v-card-text>
                <v-divider class="mt-5"></v-divider>
                <v-card-text class="text-center mt-5">
                    <v-btn variant="text" size="small" to="/signup" color="primary" @click="createAccount">Sign up
                        <v-icon>mdi-open-in-new</v-icon></v-btn>
                </v-card-text>
            </v-card>
        </v-dialog>
    </v-col>
</template>

<script setup>
import { ref } from 'vue';
import { useMutation } from '@vue/apollo-composable';
import { USER_MUTATION_LOGIN } from '@/graphql/userQuery'
import { showToast } from '@/plugins/notificationHelper';
import authStore from '@/stores/authStore';

const model = ref({
    email: '',
    password: '',
});
const loadingBtn = ref(false);
const dialog = ref(true);

const login = () => {
    const { mutate: dtoModel } = useMutation(USER_MUTATION_LOGIN);
    loadingBtn.value = true;
    dtoModel({
        email: model.value.email,
        password: model.value.password,
    }).then(result => {
        loadingBtn.value = false;
        const isError = result.data.login.stateResult.isError;
        const message = result.data.login.stateResult.message;
        const fields = result.data.login.stateResult.fields;

        if (isError) {
            showToast(message, 'error');
            return;
        }

        authStore.dispatch("login", {
            id: fields.userId,
            token: fields.token,
            useRedirect: true
        });

    }).catch(error => {
        loadingBtn.value = false;
        showToast("the server is not connecting try again", 'error');
    });
}

</script>