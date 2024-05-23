import { useToast } from 'vue-toast-notification';

export const showToast = (message, type = 'success', position = 'top-right') => {
    const $toast = useToast();
    $toast.open({
        message: message,
        type: type,
        position: position,
    });
};