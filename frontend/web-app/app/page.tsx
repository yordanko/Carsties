import Listings from "./auctions/Listings";
import Listing from "./auctions/Listings";

export default function Home() {
  return (
    <div>
      <h3 className='text-3xl font-semibold'>
      <Listings />
      </h3>
    </div>
  );
}
