import { createRouter, createWebHistory } from 'vue-router'
import authStore from '@/stores/authStore';

const routes = [
  {
    path: '/',
    component: () => import('@/layouts/default/Default.vue'),
    children: [
      {
        path: '',
        name: 'loginView',
        component: () => import('@/views/loginView/Index.vue'),
        meta: {
          requiredAuth: false,
          title: 'Login'
        },
      },
    ],
  },
  {
    path: '/signup',
    component: () => import('@/layouts/default/Default.vue'),
    children: [
      {
        path: '',
        name: 'signupView',
        component: () => import('@/views/signUpView/Index.vue'),
        meta: {
          requiredAuth: false,
          title: 'Sign Up'
        },
      },
    ],
  },
  {
    path: '/profile',
    component: () => import('@/layouts/default/Default.vue'),
    children: [
      {
        path: '',
        name: 'profileView',
        component: () => import('@/views/profileView/Index.vue'),
        meta: {
          requiredAuth: true,
          title: 'Profile'
        },
      },
      {
        path: '/profile/security',
        name: 'security',
        component: () => import('@/components/profile/Security.vue'),
        meta: {
          requiredAuth: true,
          title: 'Security'
        },
      },
    ],
  },
  {
    path: '/messages',
    component: () => import('@/layouts/default/Default.vue'),
    children: [
      {
        path: '',
        name: 'messageView',
        component: () => import('@/views/messageView/Index.vue'),
        meta: {
          requiredAuth: true,
          title: 'Messages'
        },
      },
    ],
  }
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes,
})

router.beforeEach((to, from, next) => {
  const isAuthenticated = authStore.getters.isAuthenticated;
  console.log('Is Authenticated:', isAuthenticated);  // Debugging line

  document.title = `I Chat | ${to.meta.title || 'I Chat'}`;

  if (isAuthenticated && (to.name === 'loginView' || to.name === 'signupView')) {
    next('/profile');
  } else if (to.meta.requiredAuth && !isAuthenticated) {
    next('/login');
  } else {
    next();
  }
});

export default router;
