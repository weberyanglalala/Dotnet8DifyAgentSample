const app = Vue.createApp({
        data() {
            return {
                productTitle: '',
                productDescription: '',
                productImageId: '',
                imagePreview: null,
                imagePreviewStatus: false,
                productImagesUrl: '',
                difyCreateButtonStatus: false,
                isLoading: false
            };
        },
        methods: {
            generateProductDetailsByDify() {
                this.showLoading();
                fetch("api/Dify/CreateProductDetail", {
                    method: 'POST',
                    body: JSON.stringify({
                        "product_name": this.productTitle
                    }),
                    headers: {
                        'Content-Type': 'application/json'
                    }
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.isSuccess) {
                            this.productDescription = data.body.description;
                            this.productImageId = data.body.prompt_id;
                            this.imagePreviewStatus = true
                        }
                    })
                    .catch(error => {
                        console.error('Error:', error);
                    })
                    .finally(() => {
                        this.hideLoading();
                    });
            },
            showLoading() {
                this.isLoading = true;
            },
            hideLoading() {
                this.isLoading = false;
            },
        },
        watch: {
            productTitle: function (val) {
                this.difyCreateButtonStatus = val.length >= 5 && val.length <= 30;
            }
        }
    })
;

app.mount('#app');