const app = Vue.createApp({
    data() {
        return {
            productTitle: '',
            productDescription: '',
            productCategory: '',
            productImageId: '',
            imagePreview: null,
            imagePreviewStatus: false,
            productImages: []
        };
    },
    methods: {}
});

app.mount('#app');