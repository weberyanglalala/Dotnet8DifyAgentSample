const homeVueObject = {
    data() {
        return {
            date: null
        };
    }
}
const app = Vue.createApp(homeVueObject);


app.use(PrimeVue.Config, {
    theme: {
        preset: PrimeVue.Themes.Aura
    }
});

app.component('p-datepicker', PrimeVue.DatePicker);

app.mount('#app');