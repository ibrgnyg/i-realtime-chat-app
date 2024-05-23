<template>
  <v-app-bar flat>
    <v-app-bar-title>
      i - chat
    </v-app-bar-title>

    <template v-slot:append>
      <v-btn v-if="!isAuth" variant="outlined" size="small" to="/signup">Sign up</v-btn>
      <span style="margin: 0 8px;"></span>
      <v-btn v-if="!isAuth" variant="outlined" size="small" to="/login">Login</v-btn>
      <span style="margin: 0 8px;"></span>
      <v-btn v-if="isAuth" variant="text" size="small" to="/profile">{{ authStore.getters.currentUser.user.userName }}</v-btn>
    </template>

  </v-app-bar>
</template>

<script setup>
import authStore from '@/stores/authStore';
import { ref, watch } from 'vue';

const isAuth = ref(false);

watch(() => {
  isAuth.value = authStore.getters.isAuthenticated;
});

</script>
