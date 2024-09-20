const app = Vue.createApp({
    data() {
        return {
            productTitle: '',
            productDescription: '',
            productImageId: '',
            imagePreview: null,
            imagePreviewStatus: false,
        };
    },
    methods: {
    }
});

app.mount('#app');