<template>
    <v-col class="text-center">
        <v-dialog v-model="dialog" persistent max-width="500px" height="500">
            <v-card>
                <v-card-title> i-chat | Sign up</v-card-title>
                <v-divider></v-divider>
                <v-card-text>
                    <v-text-field v-model="model.email" label="E-Posta" variant="outlined" required type="email">
                    </v-text-field>
                    <v-text-field v-model="model.userName" label="UserName" variant="outlined" required type="text">
                    </v-text-field>
                    <v-text-field v-model="model.password" label="Şifre" variant="outlined" type="password" required>
                    </v-text-field>
                    <v-btn variant="outlined" :loading="loadingBtn" v-on:click="signup">Sign up</v-btn>
                </v-card-text>
                <v-divider class="mt-5"></v-divider>
                <v-card-text class="text-center mt-5">
                    <v-btn variant="text" size="small" color="primary" to="/">Do you have an account? Log in now.
                    </v-btn>
                </v-card-text>
            </v-card>
        </v-dialog>
    </v-col>
</template>


<script setup>
import { ref } from 'vue';
import { useMutation } from '@vue/apollo-composable';
import { USER_MUTATION_SIGNUP } from '@/graphql/userQuery'
import { showToast } from '@/plugins/notificationHelper';
import router from '@/router';

const model = ref({
    email: '',
    userName: '',
    password: '',
});
const loadingBtn = ref(false);
const dialog = ref(true);


const signup = () => {
    const { mutate: dtoModel } = useMutation(USER_MUTATION_SIGNUP);

    loadingBtn.value = true;

    if (model.value.email == '' || model.value.userName == '' || model.value.password == '') {
        loadingBtn.value = false;
        showToast("required fields", 'error');
        return;
    }
    dtoModel({
        email: model.value.email,
        userName: model.value.userName,
        password: model.value.password,
    }).then(result => {
        loadingBtn.value = false;
        const isError = result.data.register.stateResult.isError;
        const message = result.data.register.stateResult.message;

        if (isError) {
            showToast(message, 'error');
            return;
        }
        showToast(message);
    }).catch(error => {
        loadingBtn.value = false;
        showToast("the server is not connecting try again", 'error');
    });
}

</script>