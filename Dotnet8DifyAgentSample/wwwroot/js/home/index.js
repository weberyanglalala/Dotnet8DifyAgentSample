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
            };
        },
        methods: {
            generateProductDetailsByDify() {
                fetch("api/Dify/CreateWorkflow", {
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
                    });
            }
        },
        watch: {
            productTitle: function (val) {
                this.difyCreateButtonStatus = val.length >= 5 && val.length <= 30;
            }
        }
    })
;

app.mount('#app');