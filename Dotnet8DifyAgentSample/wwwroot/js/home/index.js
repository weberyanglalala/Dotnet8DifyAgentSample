const app = Vue.createApp({
    data() {
        return {
            productTitle: '',
            productDescription: '',
            productCategory: '',
            productImageId: '',
            imagePreview: null,
            imagePreviewStatus: false,
        };
    },
    methods: {
    }
});

app.mount('#app');